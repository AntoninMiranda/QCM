-- Création de la table Users
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "FirstName" VARCHAR(100),
    "LastName" VARCHAR(100),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Username" ON "Users" ("Username");

-- Création de la table Scores
CREATE TABLE IF NOT EXISTS "Scores" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "QcmType" VARCHAR(100) NOT NULL,
    "Category" VARCHAR(100),
    "TotalQuestions" INTEGER NOT NULL,
    "CorrectAnswers" INTEGER NOT NULL,
    "IncorrectAnswers" INTEGER NOT NULL,
    "Percentage" DOUBLE PRECISION NOT NULL,
    "TimeSpentSeconds" INTEGER NOT NULL,
    "CompletedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "Details" TEXT,
    CONSTRAINT "FK_Scores_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_Scores_UserId" ON "Scores" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_Scores_CompletedAt" ON "Scores" ("CompletedAt");
CREATE INDEX IF NOT EXISTS "IX_Scores_UserId_QcmType" ON "Scores" ("UserId", "QcmType");

-- Table de suivi des migrations EF Core
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" VARCHAR(150) NOT NULL PRIMARY KEY,
    "ProductVersion" VARCHAR(32) NOT NULL
);

-- Enregistrement des migrations
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20251126000000_AddUserAuthentication', '9.0.0') 
ON CONFLICT DO NOTHING;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20251126210000_AddScoreTable', '9.0.0') 
ON CONFLICT DO NOTHING;

