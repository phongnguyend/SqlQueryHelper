﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace SqlServerQueryHelper.EntityFrameworkCore;

internal static class MigrationBuilderExtensions
{
    public static void CreateIndexIfNotExists(this MigrationBuilder migrationBuilder, string tableName, string indexName, string script)
    {
        var sql = SqlQueryGenerator.CreateIndexIfNotExists(tableName, indexName, script);
        migrationBuilder.Sql(sql);
    }

    public static void CreateIndexIfNotExists(this MigrationBuilder migrationBuilder, string script)
    {
        var sql = SqlQueryGenerator.CreateIndexIfNotExists(script);
        migrationBuilder.Sql(sql);
    }

    public static void DropIndexIfExists(this MigrationBuilder migrationBuilder, string tableName, string indexName)
    {
        var sql = SqlQueryGenerator.DropIndexIfExists(tableName, indexName);
        migrationBuilder.Sql(sql);
    }
}
