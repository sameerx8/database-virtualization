namespace ServiceVirtualization.Database.SqlClient {
    public class Error
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public int State { get; set; }

        public int Severity { get; set; }

        public int LineNumber { get; set; }

        public string Server { get; set; }

        public string ProcedureName { get; set; }
    }
}