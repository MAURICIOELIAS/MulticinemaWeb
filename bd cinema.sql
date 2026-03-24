-- Base de Datos
CREATE DATABASE MulticinemaDB;
GO

USE MulticinemaDB;
GO

--Tabla de USUARIOS
--se maneja la bd tanto para los registrados asi como los invitados
CREATE TABLE Usuarios (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(100) NOT NULL,
    Dui NVARCHAR(10),
    Telefono NVARCHAR(15),
    PasswordHash NVARCHAR(255) NULL, 
    EsInvitado BIT DEFAULT 0, -- 
    FechaRegistro DATETIME DEFAULT GETDATE()
);
GO

--tabla peli
CREATE TABLE Peliculas (
    IdPelicula INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(150) NOT NULL,
    Sinopsis NVARCHAR(MAX),
    DuracionMinutos INT NOT NULL, -- 
    Clasificacion NVARCHAR(10), -- 
    PosterUrl NVARCHAR(500), -- Link a la imagen
    TrailerUrl NVARCHAR(500), -- Link al video
    Estado NVARCHAR(20) CHECK (Estado IN ('Estreno', 'Cartelera', 'Proximamente'))
);
GO

-- Tabla de salas
CREATE TABLE Salas (
    IdSala INT IDENTITY(1,1) PRIMARY KEY,
    NombreSala NVARCHAR(50) NOT NULL, -- 
    CapacidadTotal INT NOT NULL,
    -- se guarda el mapa en formato JSON o texto para dibujarlo en el frontend
    
    DistribucionAsientos NVARCHAR(MAX) 
);
GO

-- Tabla de funciones
-- Relaciona Película + Sala + Hora
CREATE TABLE Funciones (
    IdFuncion INT IDENTITY(1,1) PRIMARY KEY,
    IdPelicula INT NOT NULL,
    IdSala INT NOT NULL,
    FechaHoraInicio DATETIME NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL,
    CONSTRAINT FK_Funciones_Peliculas FOREIGN KEY (IdPelicula) REFERENCES Peliculas(IdPelicula),
    CONSTRAINT FK_Funciones_Salas FOREIGN KEY (IdSala) REFERENCES Salas(IdSala)
);
GO

--  Tabla de boleto
CREATE TABLE Boletos (
    IdBoleto INT IDENTITY(1,1) PRIMARY KEY,
    IdFuncion INT NOT NULL,
    IdUsuario INT NOT NULL, -- 
    AsientoCodigo NVARCHAR(10) NOT NULL,  
    PrecioPagado DECIMAL(10, 2) NOT NULL,
    CodigoQR NVARCHAR(MAX), --  
    Estado NVARCHAR(20) DEFAULT 'Pagado', 
    FechaCompra DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT FK_Boletos_Funciones FOREIGN KEY (IdFuncion) REFERENCES Funciones(IdFuncion),
    CONSTRAINT FK_Boletos_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);
GO

USE MulticinemaDB;
GO


INSERT INTO Peliculas (Titulo, Sinopsis, DuracionMinutos, Clasificacion, PosterUrl, TrailerUrl, Estado)
VALUES (
    'Five Nights at Freddys 2', 
    'Un guardia de seguridad comienza a trabajar en Freddy Fazbears Pizza. Mientras pasa su primera noche, se da cuenta de que el turno nocturno no será tan fácil.', 
    110, 
    'B', 
    'https://ejemplo.com/poster-fnaf2.jpg', 
    'https://youtube.com/trailer-fnaf2', 
    'Estreno'
);

-- 2. inserta una Sala
INSERT INTO Salas (NombreSala, CapacidadTotal, DistribucionAsientos)
VALUES ('Sala 1 - Macro XE', 100, '{"filas": 10, "columnas": 10}'); -- JSON simulado

-- 3. Inserte una Función (hora de la pelicula en la sala)
--Aquí se usa los IDs que se generaron arriba. Asumimos que son los ID 1.
INSERT INTO Funciones (IdPelicula, IdSala, FechaHoraInicio, Precio)
VALUES (1, 1, '2026-01-08 18:00:00', 5.75);

--  Insertamos un Usuario (Tú)
INSERT INTO Usuarios (NombreCompleto, Correo, Dui, Telefono, EsInvitado)
VALUES ('Gerson Mauricio', 'gerson@email.com', '01234567-8', '7000-0000', 0);

--  --simulacion compra de boletos 
INSERT INTO Boletos (IdFuncion, IdUsuario, AsientoCodigo, PrecioPagado, CodigoQR, Estado)
VALUES (1, 1, 'F5', 5.75, 'QR_GENERADO_XYZ_123', 'Pagado');
GO

SELECT 
    b.IdBoleto,
    u.NombreCompleto AS Cliente,
    p.Titulo AS Pelicula,
    s.NombreSala AS Sala,
    b.AsientoCodigo AS Asiento,
    f.FechaHoraInicio,
    b.PrecioPagado
FROM Boletos b
JOIN Usuarios u ON b.IdUsuario = u.IdUsuario
JOIN Funciones f ON b.IdFuncion = f.IdFuncion
JOIN Peliculas p ON f.IdPelicula = p.IdPelicula
JOIN Salas s ON f.IdSala = s.IdSala;

USE MulticinemaDB;
GO


ALTER TABLE Usuarios ADD Rol NVARCHAR(20) DEFAULT 'Cliente';
GO

-- usuario admin

UPDATE Usuarios
SET Rol = 'Admin'
WHERE Correo = 'gerson@email.com'; 

-- 3. Verificamos
SELECT * FROM Usuarios;

USE MulticinemaDB;
GO


-- Correo: admin@multicinema.com
-- Contraseña: admin123
INSERT INTO Usuarios (NombreCompleto, Correo, PasswordHash, Rol, Dui, Telefono, EsInvitado, FechaRegistro)
VALUES (
    'Administrador Principal', 
    'admin@multicinema.com', 
    'admin123', 
    'Admin',    
    '00000000-0', 
    '2222-0000', 
    0, 
    GETDATE()
);
GO
SELECT * FROM Usuarios;
SELECT * FROM Peliculas

USE MulticinemaDB;
GO


ALTER TABLE Peliculas
ALTER COLUMN PosterUrl NVARCHAR(MAX);
GO


ALTER TABLE Peliculas
ALTER COLUMN TrailerUrl NVARCHAR(MAX);
GO