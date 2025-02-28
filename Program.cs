using System.Text.Json;

namespace NotifyNotes
{
    internal class Program
    {
        static void Main()
        {
            Dictionary<Guid, Note> notes = LoadNotesFromFile();

            while (true)
            {
                PrintNotes(notes);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("{0,-36} {1}", "Commands", "Description");
                Console.WriteLine(new string('-', 60));
                Console.WriteLine("{0,-36} {1}", ":create", "Creates new note");
                Console.WriteLine("{0,-36} {1}", ":update", "Updates existing note");
                Console.WriteLine("{0,-36} {1}", ":delete", "Deletes note");
                Console.WriteLine(new string('-', 60));
                Console.Write(":");
                string? command = Console.ReadLine();

                if (command == null)
                {
                    throw new ArgumentNullException(command);
                }

                if (command == "create")
                {
                    Console.Write("Note: ");
                    string? newNote = Console.ReadLine() ?? throw new ArgumentNullException(command);

                    notes.Add(Guid.NewGuid(), new Note(newNote));
                    SaveNotesToFile(notes);
                    Console.Clear();
                }
            }
        }

        static void PrintNotes(Dictionary<Guid, Note> notes)
        {
            Console.WriteLine("{0,-36} {1}", "Note", "Date");
            Console.WriteLine(new string('-', 60));
            foreach (var note in notes)
            {
                Console.WriteLine("{0,-36} {1}", note.Value.NoteText, note.Value.Date);
            }
        }

        static void SaveNotesToFile(Dictionary<Guid, Note> notes)
        {
            string json = JsonSerializer.Serialize(notes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("notes.json", json);
        }

        static Dictionary<Guid, Note> LoadNotesFromFile()
        {
            if (!File.Exists("notes.json"))
            {
                return [];
            }

            string json = File.ReadAllText("notes.json");
            return JsonSerializer.Deserialize<Dictionary<Guid, Note>>(json) ?? [];
        }
    }

    internal class Note(string noteText)
    {
        public string NoteText { get; set; } = noteText;
        public DateTime Date { get; set; } = DateTime.Now;

        public void UpdateNoteText(string newNoteText)
        {
            NoteText = newNoteText;
        }
    }
}
