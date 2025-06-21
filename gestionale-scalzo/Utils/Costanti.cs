namespace gestionale_scalzo.Utils
{
    public class Costanti
    {
        /// <summary>
        /// ADMIN
        /// </summary>
        public const string Amministratore = "Amministratore";
        
        /// <summary>
        /// BO
        /// </summary>
        public const string UtenteBO = "Utente Backoffice";
        
        /// <summary>
        /// Agente
        /// </summary>
        public const string Agente = "Agente";
        
        /// <summary>
        /// Stato iniziale della pratica, appena l'agente la carica
        /// </summary>
        public const int PraticaNuova = 1;

        /// <summary>        
        /// /// Stato messo dal BO se manca qualcosa
        /// </summary>
        public const int PraticaSospesa = 2;

        /// <summary>
        /// Stato dopo che il backoffice ha gestito il nuovo contratto        
        /// </summary>
        public const int PraticaGestita = 3;

        /// <summary>
        /// Stato dopo che la pratica è stata inviata al fornitore
        /// </summary>
        public const int PraticaInviata = 4;

        /// <summary>
        /// Stato dopo il KO del fornitore
        /// </summary>
        public const int PraticaRifiutata = 5;

        /// <summary>
        /// Stato dopo l'ok del fornitore
        /// </summary>
        public const int PraticaOKPagabile = 6;

        /// <summary>
        /// Stato storno
        /// </summary>
        public const int PraticaStornata = 7;

        /// <summary>
        /// Stato iniziale della comunicazione
        /// </summary>
        public const int ComunicazioneNonElaborata = 1;
        
        /// <summary>
        /// Il job che invia le comunicazioni è passato ed è tutto ok 
        /// </summary>
        public const int ComunicazioneInviata = 2;
        
        /// <summary>
        /// Il job è passato ma c'è stato un problema nell'invio
        /// </summary>
        public const int ComunicazioneNonInviata = 3;

        /// <summary>
        /// Comunicazione sul portale ancora non letta
        /// </summary>
        public const int ComunicazioneNonLetta = 0;

        /// <summary>
        /// Comunicazione sul portale visualizzata
        /// </summary>
        public const int ComunicazioneLetta = 1;
        
        /// <summary>
        /// Operazione riuscita nel db
        /// </summary>
        public const int DatabaseOK = 1024;
        
        /// <summary>
        /// Operazione non riuscita
        /// </summary>
        public const int KO = -1;
        
        /// <summary>
        /// Richiesta del client non valida
        /// </summary>
        public const int RichiestaNonValida = 0;

    }
}
