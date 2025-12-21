/**
 * API Error Class
 * Standardized error handling
 */
export class ApiError extends Error {
  public status?: number;
  public errors?: string[];
  public code?: string;

  constructor(message: string, status?: number, errors?: string[], code?: string) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.errors = errors;
    this.code = code;
    
    // Maintains proper stack trace for where our error was thrown
    if (Error.captureStackTrace) {
      Error.captureStackTrace(this, ApiError);
    }
  }

  /**
   * Check if error is authentication related
   */
  isAuthenticationError(): boolean {
    return this.status === 401;
  }

  /**
   * Check if error is authorization related
   */
  isAuthorizationError(): boolean {
    return this.status === 403;
  }

  /**
   * Check if error is client error (4xx)
   */
  isClientError(): boolean {
    return this.status !== undefined && this.status >= 400 && this.status < 500;
  }

  /**
   * Check if error is server error (5xx)
   */
  isServerError(): boolean {
    return this.status !== undefined && this.status >= 500;
  }
}

