namespace OData2Poco.TextTransform
{
 public    class FluentTextTemplate : FluentTextTemplate<FluentTextTemplate>
    {
        public static implicit operator string(FluentTextTemplate ft)
        {
            return ft.ToString();
        }

    }
}


