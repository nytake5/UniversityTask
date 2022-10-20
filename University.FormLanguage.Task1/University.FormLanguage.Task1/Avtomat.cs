using System.Text;

namespace University.FormLanguage.Task1;

public class Avtomat
{
    public List<char> Alphabet { get; set; }
    public List<State> States { get; set; }
    public List<State> StartStates { get; set; } = new List<State>();
    public bool IsEps { get; set; }


    public void InitialiseAvtomatFromFile(string filename)
    {
        using (StreamReader reader = new StreamReader(filename))
        {
            Alphabet = new List<char>(reader.ReadLine()?.Split(' ').Select(st => st.ToCharArray().First()));
            string[] inputNodes = reader.ReadLine().Split(' ');
            States = new List<State>();
            //Input nodes
            foreach (string s in inputNodes)
            {
                State state = new State();
                if (s.Contains("*"))
                {
                    state.IsAcceptState = true;
                    state.Name = s[1..s.Length];
                    States.Add(state);
                    continue;
                }
                if (s.Contains("->"))
                {
                    state.IsStartState = true;
                    state.Name = s[2..s.Length];
                    States.Add(state);
                    StartStates.Add(state);
                    continue;
                }
                States.Add(new State()
                {
                    Name = s
                });
            }
            
            //Input transitions
            string? line;
            while ((line = reader.ReadLine()) != null) 
            {
                foreach (State state in States)
                {
                    if (state.Name == line[0..line.IndexOf(":", StringComparison.Ordinal)])
                    {
                        int start = line.IndexOf(":", StringComparison.Ordinal) + 2;
                        int end = line.IndexOf(";",
                            StringComparison.Ordinal) - 1;
                        foreach (string str in line[start .. end].Split(" "))
                        {
                            string[] pair = str.Split('/');
                            foreach (string s in pair[1].Split(","))
                            {
                                state.Transitions.Add(new KeyValuePair<string, State>(pair[0], States.First(x => x.Name.Equals(s))));                            
                            }
                        }

                        if (line[^1] != ';')
                        {
                            string tempLineEps =
                                line[(line.IndexOf(";", StringComparison.CurrentCulture) + 2) .. line.Length];
                            foreach (string epsState in tempLineEps.Split(","))
                            {
                                state.EpsMove.Add(States.First(x => x.Name.Equals(epsState)));
                                IsEps = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public void PrintAvtomat()
    {
        
        Alphabet.ForEach(symbol => Console.Write($"\t\t{symbol}"));
        Console.WriteLine();
        States.ForEach(state =>
        {
            StringBuilder line = new StringBuilder();
            if (state.IsAcceptState)
            {
                line.Append("*");
            }

            if (state.IsStartState)
            {
                line.Append("->");
            }
            line.Append($"\t{state.Name}\t");
            foreach (string key in state.Transitions.Select(x => x.Key).ToHashSet())
            {
                foreach (KeyValuePair<string,State> pair in state.Transitions.Where(x => x.Key.Equals(key)))
                {
                    line.Append($"{pair.Value.Name} ");
                }
                line.Append("\t\t");
            }

            line.Append($" ; ");

            foreach (State epsState in state.EpsMove)
            {
                line.Append(epsState.Name + " ");
            }

            line.Append(Environment.NewLine);
            Console.WriteLine(line);
        });
    }

    public IEnumerable<bool?> Run(List<char> inputWord, int currentSymbol = 0, List<State> currentStates = null)
    {
        if (IsEps)
        {
            currentStates = EpsMove(currentStates ?? StartStates);
        }
        if (currentSymbol >= inputWord.Count)
        {
            foreach (var state in currentStates)
            {
                if (state.IsAcceptState)
                {
                    yield return true;
                    yield break;
                }
            }
            yield return false;
            yield break;
        }
        List<State> newCurrentStates = new List<State>(StartStates);
        if (currentStates != null)
        {
            StartStates = currentStates;
            newCurrentStates = new List<State>(StartStates);
        }
        foreach (State state in StartStates)
        {
            newCurrentStates.Remove(state);
            IEnumerable<State> transStates = state.Transitions.Where(x => x.Key.Equals(inputWord[currentSymbol].ToString()))
                .Select(x => x.Value).ToList();
            foreach (State transState in transStates)
            {
                Console.WriteLine($"Current state: {state.Name}, Current symbol: {inputWord[currentSymbol]} " +
                                  $"Transition to state {transState.Name}");

                if (transState == null)
                {
                    yield return false;
                    yield break;
                }

                if (transState.IsAcceptState && currentSymbol == (inputWord.Count - 1))
                {
                    yield return true;
                    yield break;
                }

                newCurrentStates.Add(transState);
            }

            newCurrentStates = newCurrentStates.ToHashSet().ToList();
            
            foreach (bool? flag in Run(inputWord, currentSymbol + 1, newCurrentStates))
            {
                yield return flag;
            }
        }
    }


    private List<State> EpsMove(List<State> currentStates)
    {
        List<State> result = new List<State>(currentStates);
        foreach (State state in currentStates)
        {
            foreach (State epsState in state.EpsMove)
            {
                result.Add(epsState);
            }
        }

        return result.ToHashSet().ToList();
    }
}