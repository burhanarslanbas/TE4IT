package com.te4it.mobile.presentation.screens.education

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.viewinterop.AndroidView

import com.te4it.mobile.domain.model.education.ContentType
import com.te4it.mobile.domain.model.education.CourseContent

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ContentScreen(
    courseId: String,
    contentId: String,
    viewModel: com.te4it.mobile.presentation.viewmodel.ContentPlayerViewModel,
    onNavigateBack: () -> Unit = {}
) {
    val content by viewModel.content.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()

    LaunchedEffect(courseId, contentId) {
        viewModel.loadContent(courseId, contentId)
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(content?.title ?: "İçerik") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Geri")
                    }
                }
            )
        }
    ) { padding ->
        Box(modifier = Modifier.padding(padding).fillMaxSize()) {
             if (isLoading) {
                 CircularProgressIndicator(modifier = Modifier.align(androidx.compose.ui.Alignment.Center))
             } else if (error != null) {
                 Text("Hata: $error", modifier = Modifier.align(androidx.compose.ui.Alignment.Center))
             } else {
                 content?.let { currentContent ->
                     ContentDetailView(content = currentContent)
                 }
             }
        }
    }
}

@Composable
fun ContentDetailView(content: CourseContent) {
    Column(modifier = Modifier.fillMaxSize().padding(16.dp)) {
        
        when (content.type) {
            ContentType.VIDEO -> {
                Text("Video İçeriği", style = MaterialTheme.typography.labelLarge)
                Spacer(modifier = Modifier.height(8.dp))
                
                if (content.embedUrl != null) {
                    val context = androidx.compose.ui.platform.LocalContext.current
                    
                    // Extract video ID from URL
                    val videoId = remember(content.embedUrl) {
                        when {
                            content.embedUrl!!.contains("/embed/") -> {
                                content.embedUrl!!.substringAfter("/embed/").substringBefore("?")
                            }
                            content.embedUrl!!.contains("watch?v=") -> {
                                content.embedUrl!!.substringAfter("watch?v=").substringBefore("&")
                            }
                            content.embedUrl!!.contains("youtu.be/") -> {
                                content.embedUrl!!.substringAfter("youtu.be/").substringBefore("?")
                            }
                            else -> null
                        }
                    }
                    
                    if (videoId != null) {
                        android.util.Log.d("ContentScreen", "Loading YouTube video ID: $videoId")
                        
                        // WebView with iframe approach (most reliable)
                        AndroidView(
                            factory = { ctx ->
                                android.webkit.WebView(ctx).apply {
                                    // Enable debugging in debug builds
                                    if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.KITKAT) {
                                        android.webkit.WebView.setWebContentsDebuggingEnabled(true)
                                    }
                                    
                                    settings.apply {
                                        javaScriptEnabled = true
                                        domStorageEnabled = true
                                        databaseEnabled = true
                                        mediaPlaybackRequiresUserGesture = false
                                        allowFileAccess = true
                                        allowContentAccess = true
                                        loadsImagesAutomatically = true
                                        mixedContentMode = android.webkit.WebSettings.MIXED_CONTENT_COMPATIBILITY_MODE
                                        useWideViewPort = true
                                        loadWithOverviewMode = true
                                        setSupportMultipleWindows(false)
                                        javaScriptCanOpenWindowsAutomatically = false
                                        
                                        // Set modern user agent
                                        userAgentString = "Mozilla/5.0 (Linux; Android ${android.os.Build.VERSION.RELEASE}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36"
                                    }
                                    
                                    // Enable cookies
                                    val cookieManager = android.webkit.CookieManager.getInstance()
                                    cookieManager.setAcceptCookie(true)
                                    cookieManager.setAcceptThirdPartyCookies(this, true)
                                    
                                    // Comprehensive WebViewClient
                                    webViewClient = object : android.webkit.WebViewClient() {
                                        override fun onPageStarted(view: android.webkit.WebView?, url: String?, favicon: android.graphics.Bitmap?) {
                                            android.util.Log.d("ContentScreen", "WebView page started: $url")
                                        }
                                        
                                        override fun onPageFinished(view: android.webkit.WebView?, url: String?) {
                                            android.util.Log.d("ContentScreen", "WebView page finished: $url")
                                        }
                                        
                                        override fun onReceivedError(
                                            view: android.webkit.WebView?,
                                            request: android.webkit.WebResourceRequest?,
                                            error: android.webkit.WebResourceError?
                                        ) {
                                            android.util.Log.e("ContentScreen", "WebView error: ${error?.description} code=${error?.errorCode}")
                                        }
                                        
                                        override fun onReceivedHttpError(
                                            view: android.webkit.WebView?,
                                            request: android.webkit.WebResourceRequest?,
                                            errorResponse: android.webkit.WebResourceResponse?
                                        ) {
                                            android.util.Log.e("ContentScreen", "WebView HTTP error: ${errorResponse?.statusCode}")
                                        }
                                        
                                        override fun shouldOverrideUrlLoading(
                                            view: android.webkit.WebView?,
                                            request: android.webkit.WebResourceRequest?
                                        ): Boolean {
                                            android.util.Log.d("ContentScreen", "URL loading: ${request?.url}")
                                            return false
                                        }
                                        
                                        override fun onRenderProcessGone(
                                            view: android.webkit.WebView?,
                                            detail: android.webkit.RenderProcessGoneDetail?
                                        ): Boolean {
                                            android.util.Log.e("ContentScreen", "WebView render process gone, recovering...")
                                            return true
                                        }
                                    }
                                    
                                    // Comprehensive WebChromeClient
                                    webChromeClient = object : android.webkit.WebChromeClient() {
                                        override fun onConsoleMessage(cm: android.webkit.ConsoleMessage?): Boolean {
                                            cm?.let {
                                                android.util.Log.d("ContentScreen", "JS Console [${it.messageLevel()}] ${it.message()} -- ${it.sourceId()}:${it.lineNumber()}")
                                            }
                                            return true
                                        }
                                        
                                        override fun onProgressChanged(view: android.webkit.WebView?, newProgress: Int) {
                                            if (newProgress % 25 == 0) {
                                                android.util.Log.d("ContentScreen", "WebView progress: $newProgress%")
                                            }
                                        }
                                    }
                                    
                                    // Use software rendering for emulator compatibility
                                    setLayerType(android.view.View.LAYER_TYPE_SOFTWARE, null)
                                    
                                    // Load iframe HTML
                                    val iframeHtml = """
                                        <!DOCTYPE html>
                                        <html>
                                        <head>
                                            <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no">
                                            <style>
                                                body, html { margin: 0; padding: 0; width: 100%; height: 100%; overflow: hidden; background: #000; }
                                                .video-container { position: relative; width: 100%; height: 100%; }
                                                iframe { position: absolute; top: 0; left: 0; width: 100%; height: 100%; border: 0; }
                                            </style>
                                        </head>
                                        <body>
                                            <div class="video-container">
                                                <iframe
                                                    src="https://www.youtube-nocookie.com/embed/$videoId?playsinline=1&rel=0&modestbranding=1&controls=1&showinfo=0&fs=1"
                                                    frameborder="0"
                                                    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                                                    allowfullscreen>
                                                </iframe>
                                            </div>
                                        </body>
                                        </html>
                                    """.trimIndent()
                                    
                                    loadDataWithBaseURL(
                                        "https://www.youtube-nocookie.com",
                                        iframeHtml,
                                        "text/html",
                                        "UTF-8",
                                        null
                                    )
                                }
                            },
                            modifier = Modifier
                                .fillMaxWidth()
                                .aspectRatio(16f / 9f)
                        )
                    } else {
                        Text("Geçersiz video URL'si", color = MaterialTheme.colorScheme.error)
                    }
                    
                    Spacer(modifier = Modifier.height(8.dp))
                    
                    // Fallback button to open in browser
                    Button(
                        onClick = {
                            val intent = android.content.Intent(android.content.Intent.ACTION_VIEW).apply {
                                data = android.net.Uri.parse("https://www.youtube.com/watch?v=${videoId ?: ""}")
                            }
                            context.startActivity(intent)
                        },
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Text("Tarayıcıda Aç")
                    }
                } else {
                    Text("Video URL bulunamadı.")
                }
            }
            ContentType.TEXT -> {
                 Text("Ders Notları", style = MaterialTheme.typography.labelLarge)
                 Spacer(modifier = Modifier.height(8.dp))
                 // Since we have HTML content, in a real app we'd use a robust HTML renderer.
                 // For now, we will just display raw text or basic text.
                 Text(text = Utils.stripHtml(content.textContent ?: ""), style = MaterialTheme.typography.bodyLarge)
            }
            else -> {
                Text("Bu içerik tipi şimdilik desteklenmiyor: ${content.type}")
            }
        }

        Spacer(modifier = Modifier.weight(1f)) // Push button to bottom
        
        Button(
            onClick = { /* Mark as complete logic */ },
            modifier = Modifier.fillMaxWidth()
        ) {
            Text("Tamamlandı Olarak İşaretle")
        }
    }
}


object Utils {
    fun stripHtml(html: String): String {
        return android.text.Html.fromHtml(html, android.text.Html.FROM_HTML_MODE_LEGACY).toString()
    }
}
