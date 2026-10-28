using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class NotesController : Controller
{
    private readonly NoteRepository _notes;

    public NotesController(NoteRepository notes)
    {
        _notes = notes;
    }

    // GET /Notes
    public async Task<IActionResult> Index()
    {
        var notes = await _notes.GetAllAsync();
        return View(notes);
    }

    // GET /Notes/Create
    public IActionResult Create()
    {
        return View(new Note());
    }

    // POST /Notes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Note note)
    {
        if (!ModelState.IsValid)
        {
            return View(note);
        }

        await _notes.AddAsync(note);
        return RedirectToAction(nameof(Index));
    }

    // GET /Notes/Edit/5
    public async Task<IActionResult> Edit(long id)
    {
        var note = await _notes.GetByIdAsync(id);
        if (note == null)
        {
            return NotFound();
        }
        return View(note);
    }

    // POST /Notes/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Note note)
    {
        if (!ModelState.IsValid)
        {
            return View(note);
        }

        await _notes.UpdateAsync(note);
        return RedirectToAction(nameof(Index));
    }

    // GET /Notes/Delete/5
    public async Task<IActionResult> Delete(long id)
    {
        var note = await _notes.GetByIdAsync(id);
        if (note == null)
        {
            return NotFound();
        }
        return View(note);
    }

    // POST /Notes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        await _notes.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}