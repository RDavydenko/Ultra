export interface HttpResponse<T> {
  isSuccess: boolean;
  statusCode: number;
  object: T | null;
  error?: string;
}

export interface Dummy {}

export interface ResponseErrorTypes {
  isSuccess: boolean;
  messages: [
    {
      messages: [
        {
          status: number;
          text: string;
          header: string;
        }
      ];
    }
  ];
  title?: string;
}
