# Ristorante Gestionale - Full Stack App

## Descrizione del progetto

Questa applicazione full-stack è un gestionale per ristoranti che permette la gestione di menù, piatti, categorie e valutazioni tramite un'API esposta dal back-end. Il front-end è realizzato con WPF, mentre il back-end è sviluppato in C# utilizzando ASP.NET Core. Il progetto include un sistema di autenticazione basato su JWT (JSON Web Tokens) e una gestione dei ruoli degli utenti, che consente di limitare l'accesso alle API in base al tipo di utente (utente semplice e amministratore).

### Caratteristiche principali

1. **Gestione di Menù, Piatti e Categorie**:  
   Ogni menù può contenere più piatti, e ogni piatto può appartenere a più menù. I piatti sono organizzati in categorie (primi piatti, secondi piatti, contorni, ecc.).

2. **Autenticazione e autorizzazione**:  
   - Autenticazione tramite JWT.
   - Autorizzazione a seconda del ruolo dell'utente:
     - Gli utenti semplici possono visualizzare i piatti, ma non possono modificarli.
     - Gli amministratori possono modificare, aggiungere e cancellare piatti, menù e categorie.

3. **API di valutazione**:  
   I piatti hanno una valutazione media, che può essere aggiornata con nuovi voti da parte degli utenti. Le API di valutazione sono pubbliche.

### Tecnologie utilizzate

- **Back-End**: ASP.NET Core Web API
- **Front-End**: WPF (Windows Presentation Foundation)
- **Autenticazione**: JWT (JSON Web Token)
- **Database**: SQL Server

