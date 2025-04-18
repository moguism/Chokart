export class Result<T = void> {
    success: boolean;
    statusCode: number;
    error: string;
    data: T;
  
    private constructor(success: boolean, statusCode: number, error: string = null, data: T = null) {
      this.success = success;
      this.error = error;
      this.statusCode = statusCode;
      this.data = data;
    }
  
    throwIfError() {
      if (!this.success) {
        throw new Error(this.error);
      }
    }
  
    static success<T = void>(statusCode: number, data: T = null): Result<T> {
      return new Result(true, statusCode, null, data);
    } 
  
    static error<T = void>(statusCode: number, error: string = null): Result<T> {
      return new Result(false, statusCode, error);
    } 
  }
  