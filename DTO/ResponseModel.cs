namespace PokemonReviewApp.DTO
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ResponseModel(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }

}
