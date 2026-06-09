namespace OFIS.Roles
{
    public readonly struct RolePayloadValidationResult
    {
        public bool Success { get; }
        public string Message { get; }

        private RolePayloadValidationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static RolePayloadValidationResult Passed(string message)
        {
            return new RolePayloadValidationResult(true, message);
        }

        public static RolePayloadValidationResult Failed(string message)
        {
            return new RolePayloadValidationResult(false, message);
        }

        public override string ToString()
        {
            return Success ? $"PASS: {Message}" : $"FAIL: {Message}";
        }
    }
}
