using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class OperationOrderingDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Definisci l'ordine desiderato dei tag senza numeri
        var orderedTags = new List<string>
        {
            "Users",
            "Locations",
            "Contracts",
            "Clients",
            "Products",
            "Internal Docs",
            "Comunications",
            "Statistics",
            "Links",            
        };

        // Crea un nuovo OpenApiPaths per contenere i percorsi ordinati
        var sortedPaths = new OpenApiPaths();

        // Ordina i percorsi (ignora i controller, ordina per tag)
        foreach (var path in swaggerDoc.Paths.OrderBy(p =>
        {
            var operation = p.Value.Operations.FirstOrDefault().Value; // Prendi la prima operazione
            var tagName = operation?.Tags?.FirstOrDefault()?.Name ?? ""; // Prendi il tag, se esiste
            return orderedTags.IndexOf(tagName); // Restituisci l'indice del tag
        }))
        {
            // Per ogni percorso, ordina le operazioni in base ai tag
            var sortedOperations = path.Value.Operations
                .OrderBy(op =>
                {
                    var tagName = op.Value.Tags?.FirstOrDefault()?.Name ?? ""; // Prendi il tag
                    return orderedTags.IndexOf(tagName); // Restituisci l'indice del tag
                })
                .ToList();

            // Crea un nuovo OpenApiPathItem per ciascun percorso
            var newPathItem = new OpenApiPathItem();

            foreach (var operation in sortedOperations)
            {
                // Aggiungi l'operazione al percorso ordinato
                newPathItem.Operations[operation.Key] = operation.Value;
            }

            // Aggiungi il percorso ordinato alla nuova collezione
            sortedPaths.Add(path.Key, newPathItem);
        }

        // Assegna il dizionario ordinato di percorsi alla proprietà Paths
        swaggerDoc.Paths = sortedPaths;
    }
}
