using System.Linq;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Annotations;

namespace OData2Poco.V3
{
    static class VocabularyHelpersV3
    {
        private static IEdmVocabularyAnnotation? FindVocabularyAnnotation(this IEdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term)
        {
            var annotations = model.FindVocabularyAnnotations(target);
            if (annotations == null) return null;
            var annotation = annotations.FirstOrDefault(a => a.Term.Namespace == term.Namespace && a.Term.Name == term.Name);
            var result = annotation;

            return result;
        }
        //Computed and Permissions Vocabulary are not supported in OData V3
        internal static bool IsReadOnly(this IEdmModel model, IEdmProperty property) => false;
    }
}
