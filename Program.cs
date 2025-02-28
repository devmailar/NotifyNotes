using System.Text.Json;

namespace NotifyNotes
{
    internal class Program
    {
        static void Main()
        {
            Dictionary<int, Note> notes = LoadNotesFromFile();
            bool isReading = false;

            while (!isReading)
            {
                Console.WriteLine("_  _ ____ ___ _ ____ _   _ _  _ ____ ___ ____ ____ ");
                Console.WriteLine("|\\ | |  |  |  | |___  \\_/  |\\ | |  |  |  |___ [__  ");
                Console.WriteLine("| \\| |__|  |  | |      |   | \\| |__|  |  |___ ___] ");
                Console.WriteLine();

                PrintNotes(notes);
                Console.WriteLine();
                isReading = PrintCommands(notes);
            }
        }

        static void PrintNotes(Dictionary<int, Note> notes)
        {
            foreach (var note in notes)
            {
                string truncatedNote = note.Value.NoteText.Length > 30 ? string.Concat(note.Value.NoteText.AsSpan(0, 30), "...") : note.Value.NoteText;

                Console.WriteLine("[{0}] {1,-33} [{2}]", note.Key, truncatedNote, note.Value.Date);
            }
        }

        static bool PrintCommands(Dictionary<int, Note> notes)
        {
            Console.WriteLine("Controls");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine(":read   || read note");
            Console.WriteLine(":create || create new note");
            Console.WriteLine(":update || update existing note");
            Console.WriteLine(":delete || delete note");
            Console.WriteLine(new string('-', 60));
            Console.Write(":");
            string? command = Console.ReadLine();

            if (command == null)
            {
                throw new Exception(command);
            }

            string[] commandParts = command.Split(' ');

            if (command.StartsWith("read"))
            {
                if (commandParts.Length < 2)
                {
                    Console.Write("ID: ");
                    commandParts = ["read", Console.ReadLine() ?? throw new ArgumentNullException(command)];
                }

                string noteId = commandParts[1];

                if (int.TryParse(noteId, out int id) && notes.TryGetValue(id, out Note? note))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Clear();
                    Console.WriteLine($"{note.NoteText}");
                    Console.WriteLine();
                    Console.WriteLine($"{note.Date}");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to return to the main menu...");
                    Console.ReadKey();
                    Console.ResetColor();
                    Console.Clear();
                    return false;
                }
                else
                {
                    Console.WriteLine("Invalid ID or note not found.");
                    Console.Clear();
                }
            }
            else if (command.StartsWith("create"))
            {
                if (commandParts.Length < 2)
                {
                    Console.Write("Note: ");
                    commandParts = ["create", Console.ReadLine() ?? throw new ArgumentNullException(command)];
                }

                string noteText = commandParts[1];

                Random random = new();
                int randomId = random.Next(100, 1000);

                while (notes.ContainsKey(randomId))
                {
                    randomId = random.Next(100, 1000);
                }

                notes.Add(randomId, new Note(noteText));
                SaveNotesToFile(notes);
                Console.Clear();
            }
            else if (command.StartsWith("delete"))
            {
                if (commandParts.Length < 2)
                {
                    Console.Write("ID: ");
                    commandParts = ["delete", Console.ReadLine() ?? throw new ArgumentNullException(command)];
                }

                string noteId = commandParts[1];

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

            return false;
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