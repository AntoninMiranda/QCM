CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "FirstName" VARCHAR(100),
    "LastName" VARCHAR(100),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Username" ON "Users" ("Username");

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" VARCHAR(150) NOT NULL PRIMARY KEY,
    "ProductVersion" VARCHAR(32) NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20251126000000_AddUserAuthentication', '9.0.0') 
ON CONFLICT DO NOTHING;

