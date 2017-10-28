namespace CsSiteSearchDemo.Models
{
    public class QnAResponse
    {
        public Answer[] answers { get; set; }
    }

    public class Answer
    {
        public float score { get; set; }
        public int qnaId { get; set; }
        public string answer { get; set; }
        public string source { get; set; }
        public string[] questions { get; set; }
        public object[] metadata { get; set; }
    }
}