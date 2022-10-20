 // See https://aka.ms/new-console-template for more information

using University.FormLanguage.Task1;

Avtomat avtomat = new Avtomat();
avtomat.InitialiseAvtomatFromFile(@"D:\Uni\University.FormLanguage.Task1\University.FormLanguage.Task1\NewFile1.txt");
avtomat.PrintAvtomat();
var answer = avtomat.Run("aba".ToCharArray().ToList()).ToList();
if (answer.Any(x => x == true))
{
    Console.WriteLine("True");
}
else
{
    Console.WriteLine("False");
}