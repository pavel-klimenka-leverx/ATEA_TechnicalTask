﻿using CLI.Extensions;
using DataAccess;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace CLI
{
    public class App
    {
        private const char SeparatorCharacter = '-';
        private const int MaxArgumentLength = 50;

        private string? _arg1;
        private string? _arg2;
        private bool _requestedExit;
        private IRepository<ArgumentsRecord> _repository;

        public App()
        {
            _repository = new ArgumentsRepository();
            _requestedExit = false;
        }

        ~App()
        {
            _repository.Dispose();
        }

        public async Task Run()
        {
            Console.WriteLine("Welcome to ATEA Technical Task program.\n");

            while(!_requestedExit)
            {
                PrintArguments();
                await ExecuteAction(PrintMenu().Key);
                PrintSeparator();
            }
        }

        private void PrintArguments()
        {
            if (ArgumentsAreValid())
            {
                Console.WriteLine($"Current arguments: 1) '{_arg1}' 2) '{_arg2}'");
            }
        }

        private ConsoleKeyInfo PrintMenu()
        {
            Console.WriteLine("(S)et arguments");
            Console.WriteLine("(L)ist previous arguments (database)");
            if(ArgumentsAreValid())
            {
                Console.WriteLine("(A)dd arguments");
            }
            Console.WriteLine("(Q)uit");

            return Console.ReadKey(true);
        }

        private async Task ExecuteAction(ConsoleKey key)
        {
            switch(key)
            {
                case ConsoleKey.S:
                    {
                        SetArguments();
                        break;
                    }
                case ConsoleKey.A:
                    {
                        if (!ArgumentsAreValid()) goto default;
                        PrintAdditionResult(this.AddArguments(_arg1!, _arg2!));
                        break;
                    }
                case ConsoleKey.L:
                    {
                        List<ArgumentsRecord> records = await _repository.GetAll();
                        PrintDatabaseRecords(records);
                        break;
                    }
                case ConsoleKey.Q:
                    {
                        _requestedExit = true;
                        Console.WriteLine("\nGoodbye!");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("\nWrong input. Try again");
                        break;
                    }
            }
        }

        private void SetArguments()
        {
            Console.WriteLine("Enter two arguments separated by a whitespace character:");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                PrintInvalidInputMessage("there should be 2 arguments separated by a whitespace character.");
                return;
            }

            string[] inputChunks = input.Split(' ').Select(e => e.Trim()).ToArray();

            if (inputChunks.Length != 2)
            {
                PrintInvalidInputMessage("there should be 2 arguments separated by a whitespace character.");
                return;
            }

            if(inputChunks.Any(e => e.Length > MaxArgumentLength))
            {
                PrintInvalidInputMessage($"argument length shouldn't exceed the maximum length ({MaxArgumentLength})");
                return;
            }

            _arg1 = inputChunks[0];
            _arg2 = inputChunks[1];

            _repository.Insert(new ArgumentsRecord(_arg1, _arg2));
        }

        private void PrintSeparator(int length = 50)
        {
            Console.WriteLine("\n" + new string(SeparatorCharacter, length) + "\n");
        }

        private bool ArgumentsAreValid()
        {
            return _arg1 != null && _arg2 != null;
        }

        private void PrintAdditionResult(string result)
        {
            Console.WriteLine($"\nResult: {result}");
        }

        private void PrintDatabaseRecords(List<ArgumentsRecord> records)
        {
            Console.WriteLine();
            foreach (ArgumentsRecord record in records) 
            {
                Console.WriteLine($"{record.Id}) Arg1 = {record.Arg1}; Arg2 = {record.Arg2}");
            }
        }

        private void PrintInvalidInputMessage(string message)
        {
            Console.WriteLine($"\nInvalid input: {message}");
        }

    }
}
