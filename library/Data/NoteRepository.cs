using library.Models;
using Npgsql;

namespace library.Data;

// Every SQL statement about notes lives in this one class.
public class NoteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NoteRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Column order in every SELECT: note_id, title, body, created_on
    private static Note ReadNote(NpgsqlDataReader reader)
    {
        return new Note
        {
            NoteId = reader.GetInt64(0),
            Title = reader.GetString(1),
            Body = reader.IsDBNull(2) ? null : reader.GetString(2),
            CreatedOn = reader.GetFieldValue<DateOnly>(3)
        };
    }

    // READ: all notes, newest first.
    public async Task<List<Note>> GetAllAsync()
    {
        const string sql = "SELECT note_id, title, body, created_on " +
                           "FROM lending.note " +
                           "ORDER BY created_on DESC, note_id DESC;";

        var notes = new List<Note>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            notes.Add(ReadNote(reader));
        }

        return notes;
    }

    // READ: one note, or null when no note has that id.
    public async Task<Note?> GetByIdAsync(long id)
    {
        const string sql = "SELECT note_id, title, body, created_on " +
                           "FROM lending.note WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadNote(reader) : null;
    }

    // CREATE: the database chooses note_id and created_on.
    public async Task AddAsync(Note note)
    {
        const string sql = "INSERT INTO lending.note (title, body) " +
                           "VALUES (@title, @body);";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue("body", (object?)note.Body ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    // UPDATE: title and body are editable.
    public async Task UpdateAsync(Note note)
    {
        const string sql = "UPDATE lending.note " +
                           "SET title = @title, body = @body " +
                           "WHERE note_id = @note_id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue("body", (object?)note.Body ?? DBNull.Value);
        command.Parameters.AddWithValue("note_id", note.NoteId);

        await command.ExecuteNonQueryAsync();
    }

    // DELETE: remove one note.
    public async Task DeleteAsync(long id)
    {
        const string sql = "DELETE FROM lending.note WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}