using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace OData2Poco.TextTransform
{
    public interface IFluentTextTemplate<T>
    {
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        CompilerErrorCollection Errors { get; }

        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        string CurrentIndent { get; }

        /// <summary>
        /// Current transformation session
        /// </summary>
        IDictionary<string, object> Session { get; set; }

        string PopIndentText { get; set; }

        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        //FluentTextTemplate.ToStringInstanceHelper ToStringHelper { get; }

        string Build();
        string ToString();

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        T Write(string textToAppend);

        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        T Write(string format, params object[] args);

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        T WriteLine(string textToAppend);
      
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        T WriteLine(string format, params object[] args);


        T PushTabIndent(int n);

        /// <summary>
        /// Increase the indent
        /// </summary>
        T PushIndent(string indent);

        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        T PopIndent();

        /// <summary>
        /// Remove any indentation
        /// </summary>
        T ClearIndent();

        T PushSpaceIndent(int n = 4);


    }
}