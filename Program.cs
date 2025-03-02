using System.Text;
using System.Text.Json;

namespace NotifyNotes
{
    internal class Program
    {
        #region main
        static void Main()
        {
            var notes = LoadNotesFromFile();
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
        #endregion

        #region methods
        static void PrintNotes(Dictionary<int, Note> notes)
        {
            foreach (var note in notes.OrderByDescending(note => note.Value.Date))
            {
                string truncatedNote = note.Value.Text.Length > 30 ? string.Concat(note.Value.Text.AsSpan(0, 30), "...") : note.Value.Text;

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
                    Console.WriteLine($"{note.Text}");
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

                string text = string.Join(' ', commandParts.Skip(1));
                text = text.Replace(",", ", ").Replace(":", ": ").Replace(";", "; ");
                text = text.Replace("\r\n\r\n", "\n\n").Replace("\n\n", "\n\n");

                Random random = new();
                int randomId = random.Next(100, 1000);

                while (notes.ContainsKey(randomId))
                {
                    randomId = random.Next(100, 1000);
                }

                notes.Add(randomId, new Note { Text = text });
                SaveNotesToFile(notes);
                Console.Clear();
            }
            else if (command.StartsWith("update"))
            {
                if (commandParts.Length < 2)
                {
                    Console.Write("ID: ");
                    commandParts = ["update", Console.ReadLine() ?? throw new ArgumentNullException(command)];
                }

                string noteId = commandParts[1];

                if (int.TryParse(noteId, out int id) && notes.TryGetValue(id, out Note? note))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Clear();
                    Console.WriteLine($"{note.Text}");
                    Console.WriteLine();
                    Console.WriteLine($"{note.Date}");
                    Console.WriteLine();
                    Console.WriteLine("Enter new text for the note:");
                    Console.Write("> ");
                    string newNoteText = ReadLineWithDefault(note.Text) ?? throw new ArgumentNullException("newNoteText");
                    note.UpdateNoteText(newNoteText);
                    SaveNotesToFile(notes);
                    Console.ResetColor();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Invalid ID or note not found.");
                    Console.Clear();
                }
            }
            else if (command.StartsWith("delete"))
            {
                if (commandParts.Length < 2)
                {
                    Console.Write("ID: ");
                    commandParts = ["delete", Console.ReadLine() ?? throw new ArgumentNullException(command)];
                }

                string noteId = commandParts[1];

                if (int.TryParse(noteId, out int id) && notes.Remove(id))
                {
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
            string json = JsonSerializer.Serialize(notes);
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

        static string? ReadLineWithDefault(string defaultText)
        {
            var buffer = new StringBuilder(defaultText);
            int cursorPosition = defaultText.Length;
            Console.Write(defaultText);

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return buffer.ToString();
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (cursorPosition > 0)
                    {
                        buffer.Remove(cursorPosition - 1, 1);
                        cursorPosition--;
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write(buffer.ToString() + " ");
                        Console.SetCursorPosition(cursorPosition, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Delete)
                {
                    if (cursorPosition < buffer.Length)
                    {
                        buffer.Remove(cursorPosition, 1);
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write(buffer.ToString() + " ");
                        Console.SetCursorPosition(cursorPosition, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (cursorPosition > 0)
                    {
                        cursorPosition--;
                        Console.SetCursorPosition(cursorPosition, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (cursorPosition < buffer.Length)
                    {
                        cursorPosition++;
                        Console.SetCursorPosition(cursorPosition, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Home)
                {
                    cursorPosition = 0;
                    Console.SetCursorPosition(cursorPosition, Console.CursorTop);
                }
                else if (keyInfo.Key == ConsoleKey.End)
                {
                    cursorPosition = buffer.Length;
                    Console.SetCursorPosition(cursorPosition, Console.CursorTop);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    buffer.Insert(cursorPosition, keyInfo.KeyChar);
                    cursorPosition++;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(buffer.ToString() + " ");
                    Console.SetCursorPosition(cursorPosition, Console.CursorTop);
                }
            }
        }

        #endregion
    }

    internal class Note
    {
        public required string Text { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public void UpdateNoteText(string newText)
        {
            Text = newText;
        }
    }
}