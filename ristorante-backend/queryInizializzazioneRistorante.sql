        CREATE TABLE Categoria
    (
        [Id] INT NOT NULL identity(1,1) PRIMARY KEY,
        [nome] VARCHAR(100) NOT NULL
    )

        CREATE TABLE Piatto
    (
        [Id] INT NOT NULL identity(1,1) PRIMARY KEY,
        [nome] VARCHAR(50) NOT NULL,
        [descrizione] VARCHAR(255) NOT NULL,
        [prezzo] FLOAT NOT NULL,
        [categoriaId] INT, 
        foreign key (categoriaId) REFERENCES Categoria(Id) on delete set null
    )

    CREATE TABLE Ristorante
    (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [nome] VARCHAR(50) not null
    )

        CREATE TABLE Menu
    (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [nome] VARCHAR(50) not null,
        [ristoranteId] int not null,
        FOREIGN KEY (ristoranteId) REFERENCES Ristorante(Id) ON DELETE CASCADE
    )

        CREATE TABLE PiattoMenu
    (
        piattoId INT,
        menuId INT,
        PRIMARY KEY (piattoId, menuId),
        FOREIGN KEY (piattoId) REFERENCES Piatto(Id) on delete cascade,
        FOREIGN KEY (menuId) REFERENCES Menu(Id) on delete cascade
    )

    INSERT INTO Categoria (nome) VALUES 
    ('Antipasti'), 
    ('Primi Piatti'), 
    ('Secondi Piatti'), 
    ('Dolci'), 
    ('Bevande');

    INSERT INTO Piatto (nome, descrizione, prezzo, categoriaId) VALUES 
    ('Bruschetta', 'Pane tostato con pomodoro e basilico', 5.50, 1),  
    ('Spaghetti Carbonara', 'Spaghetti con uova, pancetta e pecorino', 12.00, 2),  
    ('Bistecca alla Fiorentina', 'Bistecca di manzo alla griglia', 25.00, 3),  
    ('Tiramisù', 'Dolce al mascarpone e caffè', 6.50, 4),  
    ('Acqua Naturale', 'Bottiglia da 500ml', 2.00, 5);

    INSERT INTO Ristorante (nome) VALUES 
    ('La Trattoria Italiana'),  
    ('Ristorante Da Mario'),  
    ('Osteria del Gusto');

    INSERT INTO Menu (nome, ristoranteId) VALUES  
    ('Menu Tradizionale', 1),  
    ('Menu Gourmet', 2),  
    ('Menu Degustazione', 3);

    -- Associare piatti ai menu
    INSERT INTO PiattoMenu (piattoId, menuId) VALUES  
    (1, 1),  -- Bruschetta nel Menu Tradizionale
    (2, 1),  -- Spaghetti Carbonara nel Menu Tradizionale
    (3, 2),  -- Bistecca alla Fiorentina nel Menu Gourmet
    (4, 3),  -- Tiramisù nel Menu Degustazione
    (5, 1),  -- Acqua Naturale nel Menu Tradizionale
    (5, 2),  -- Acqua Naturale nel Menu Gourmet
    (5, 3);  -- Acqua Naturale nel Menu Degustazione


    CREATE TABLE Utente
    (
        Id int primary key identity (1,1) not null,
        Email nvarchar(50) not null unique,
        PasswordHash nvarchar(255) not null
    );

    CREATE TABLE Ruolo
    (
        Id INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
        Nome NVARCHAR(50) NOT NULL UNIQUE
    );

    CREATE TABLE UtenteRuolo
    (
        UtenteId INT NOT NULL,
        RuoloId INT NOT NULL,
        PRIMARY KEY (UtenteId, RuoloId),
        FOREIGN KEY (UtenteId) REFERENCES Utente(Id),
        FOREIGN KEY (RuoloId) REFERENCES Ruolo(Id)
    );

    CREATE TABLE PiattoUtenteVoto
    (
        PiattoId INT NOT NULL,
        UtenteId INT NOT NULL,
        Voto INT NOT NULL CHECK (Voto BETWEEN 1 AND 10),
        PRIMARY KEY (PiattoId, UtenteId),
        FOREIGN KEY (PiattoId) REFERENCES Piatto(Id),
        FOREIGN KEY (UtenteId) REFERENCES Utente(Id)
    );

    insert into ruolo (nome) values ('superAdmin');
    insert into utente(email,passwordhash) values ('superAdmin@prova.com','AQAAAAIAAYagAAAAEEOoenBKf+Hd6FfY57xO9/Ik08TsH5Vi7H7+cbhDkyqyyoiWpx6sLnFC8WLiJ3ys6g=='); -- per testare il login la password è l' hash di "prova"
    insert into utenteruolo(utenteid,ruoloid) values (1,1); -- diamo il ruolo di superAdmin al primo utente inserito nel DB in questo caso all' utente con l' email: superAdmin@prova.com
    insert into utente(email,passwordhash) values ('admin@prova.com','AQAAAAIAAYagAAAAEEOoenBKf+Hd6FfY57xO9/Ik08TsH5Vi7H7+cbhDkyqyyoiWpx6sLnFC8WLiJ3ys6g=='); -- per testare il login la password è l' hash di "prova"
    insert into ruolo (nome) values ('admin');
    insert into utenteruolo(utenteid,ruoloid) values (2,2); -- diamo il ruolo di admin al secondo utente inserito nel DB in questo caso all' utente con l' email: admin@prova.com 