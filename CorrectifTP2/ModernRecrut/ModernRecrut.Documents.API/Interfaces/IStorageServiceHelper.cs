

using ModernRecrut.Documents.API.Models;

namespace ModernRecrut.Documents.Interfaces
{
    public interface IStorageServiceHelper
    {

        Task<IEnumerable<string>> ObtenirCheminFichiers(string idUtilisateur);

        Task EnregistrerFichier(Fichier fichier);

    }
}
