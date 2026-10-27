using library.Models;
using Npgsql;

namespace library.Data;

// Every SQL statement about notes lives in this one class.
public class NoteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    // ASP.NET Core hands in the data source that Program.cs registered.
    public NoteRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Copies the current row of the reader into a Note.
    // The positions 0 to 3 match the column order in every SELECT below.
    private static Note ReadNote(NpgsqlDataReader reader)
    {
        return new Note
        {
            NoteId = reader.GetString(0),
            Words = reader.GetString(1),
         
        };
    }

    // READ: every member, sorted by full name.
    public async Task<List<Note>> GetAllAsync()
    {
        const string sql = "SELECT note_id, words FROM lending.note ORDER BY words;";

        var notes = new List<Note>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            notes.Add(ReadNote(reader));
        }

        return notes;
    }

    // CREATE: insert a new note. The database chooses note_id.
    public async Task AddAsync(Note note)
    {
        const string sql = "INSERT INTO lending.note (note_id, words) " +
                           "VALUES (@note_id, @words);";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("note_id", note.NoteId);
        command.Parameters.AddWithValue("words", note.Words );

        await command.ExecuteNonQueryAsync();
    }

    // READ: one note, or null when no note has that id.
    public async Task<Note?> GetByIdAsync(string id)
    {
        const string sql = "SELECT note_id, words FROM lending.note WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadNote(reader) : null;
    }

    // UPDATE: change the three editable columns of one note.
    public async Task UpdateAsync(Note note)
    {
        const string sql = "UPDATE lending.note " +
                           "SET words = @words " +
                           "WHERE note_id = @note_id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("words", note.Words);
        command.Parameters.AddWithValue("note_id", note.NoteId);

        await command.ExecuteNonQueryAsync();
    }

    // DELETE: remove one note.
    public async Task DeleteAsync(string id)
    {
        const string sql = "DELETE FROM lending.note WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}