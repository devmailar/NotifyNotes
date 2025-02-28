using System.Text.Json;

namespace NotifyNotes
{
    internal class Program
    {
        static void Main()
        {
            Dictionary<int, Note> notes = LoadNotesFromFile();
            while (true)
            {
                Console.WriteLine("_  _ ____ ___ _ ____ _   _ _  _ ____ ___ ____ ____ ");
                Console.WriteLine("|\\ | |  |  |  | |___  \\_/  |\\ | |  |  |  |___ [__  ");
                Console.WriteLine("| \\| |__|  |  | |      |   | \\| |__|  |  |___ ___] ");
                Console.WriteLine();
                Console.WriteLine();

                PrintNotes(notes);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                PrintCommands(notes);
            }
        }

        static void PrintNotes(Dictionary<int, Note> notes)
        {
            foreach (var note in notes)
            {
                string truncatedNote = note.Value.NoteText.Length > 30 ? string.Concat(note.Value.NoteText.AsSpan(0, 30), "...") : note.Value.NoteText;

                Console.WriteLine("{0,-10} {1,-36} {2}", note.Key, truncatedNote, note.Value.Date);
            }
        }

        static void PrintCommands(Dictionary<int, Note> notes)
        {
            Console.WriteLine("Controls");
            Console.WriteLine(new string('-', 32));
            Console.WriteLine(":create || Creates new note");
            Console.WriteLine(":update || Updates existing note");
            Console.WriteLine(":delete || Deletes note");
            Console.WriteLine(new string('-', 32));
            Console.Write(":");
            string? command = Console.ReadLine();

            if (command == "create")
            {
                Console.Write("Note: ");
                string? newNote = Console.ReadLine() ?? throw new ArgumentNullException(command);

                Random random = new();
                int randomId = random.Next(100, 1000);

                while (notes.ContainsKey(randomId))
                {
                    randomId = random.Next(100, 1000);
                }

                notes.Add(randomId, new Note(newNote));
                SaveNotesToFile(notes);
                Console.Clear();
            }
            else if (command == "delete")
            {
                Console.Write("ID: ");
                string? noteId = Console.ReadLine() ?? throw new ArgumentNullException(command);

                if (int.TryParse(noteId, out int id) && notes.ContainsKey(id))
                {
                    notes.Remove(id);
                    SaveNotesToFile(notes);
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Invalid ID or note not found.");
                    Console.Clear();
                }
            }
            else if (command == "clear")
            {
                Console.Clear();
            }
            else
            {
                Console.Clear();
            }
        }

        static void SaveNotesToFile(Dictionary<int, Note> notes)
        {
            string json = JsonSerializer.Serialize(notes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("notes.json", json);
        }

        static Dictionary<int, Note> LoadNotesFromFile()
        {
            if (!File.Exists("notes.json"))
            {
                return [];
            }

            string json = File.ReadAllText("notes.json");
            return JsonSerializer.Deserialize<Dictionary<int, Note>>(json) ?? [];
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