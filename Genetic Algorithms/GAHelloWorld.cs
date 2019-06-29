using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Genetic_Algorithms
{
    public class GAHelloWorld
    {
        static private int RAND_MAX = int.MaxValue;

        /*
        private const int GA_POPSIZE = 2048;            // ga population size
        private const int GA_MAXITER = 16384;           // maximum iterations
        private const double GA_ELITRATE = 0.10;        // elitism rate
        private const double GA_MUTATIONRATE = 0.25;    // mutation rate
        private const double GA_MUTATION = RAND_MAX * GA_MUTATIONRATE;
        private const string GA_TARGET = "Hello world!";
        */

        private const int GA_POPSIZE = 2048;            // ga population size
        private const int GA_MAXITER = 16384;           // maximum iterations
        private const double GA_ELITRATE = 0.10;        // elitism rate
        private const double GA_MUTATIONRATE = 0.25;    // mutation rate
        private double GA_MUTATION = RAND_MAX * GA_MUTATIONRATE;
        private const string GA_TARGET = "The quick brown fox jumped over the lazy dog.";
        //private const string GA_TARGET = "Hello world!";

        private Random _rand = new Random();

        public GAHelloWorld()
        {
        }

        private void init_population(GAVector population, GAVector buffer)
        {
            int tsize = GA_TARGET.Length;

            for (int i = 0; i < GA_POPSIZE; ++i)
            {
                ga_struct citizen = new ga_struct();
                citizen.fitness = 0;
                citizen.str.Clear();

                for (int j=0; j<tsize; ++j)
                {
                    int charValue = (rand() % 90) + 32;
                    citizen.str.Append((char) charValue);
                }

                population.Add(citizen);
            }

            buffer.resize(GA_POPSIZE);
            //var temp = new ga_struct[GA_POPSIZE];
            //buffer = temp.ToList<ga_struct>();
        }

        private void calc_fitness(GAVector population)
        {
            string target = GA_TARGET;

            int tsize = target.Length;
            uint fitness;

            for (int i=0; i<GA_POPSIZE; ++i)
            {
                fitness = 0;
                for (int j=0; j<tsize; ++j)
                {
                    fitness += (uint)Math.Abs(population[i].str[j] - target[j]);
                }

                population[i].fitness = fitness;
            }

        }

        /*private bool fitness_sort(ga_struct x, ga_struct y)
        {
            return (x.fitness < y.fitness);
        }*/

        private void sort_by_fitness(GAVector population)
        {
            // This shows calling the Sort(Comparison(T) overload using  
            // an anonymous method for the Comparison delegate.  
            // This method treats null as the lesser of two values.
            population.Sort(delegate(ga_struct x, ga_struct y)
            {
                return x.fitness.CompareTo(y.fitness);
                /*if (x.PartName == null && y.PartName == null) return 0;
                else if (x.PartName == null) return -1;
                else if (y.PartName == null) return 1;
                else return x.PartName.CompareTo(y.PartName);*/
            });
        }

        /*private ga_struct select(GAVector population, int tournamentSize, double p)
        {
            // Select members for the tournament at random from the population
            GAVector tournamentMembers = new GAVector();
            for (int i = 0; i < tournamentSize; ++i)
            {
                int index = rand() % GA_POPSIZE;
                tournamentMembers.Add(population[index]);
            }

            // Now select from the tournament members using probability p
            //double powerBase = (1 / (1-p));
            //int maxIndex = (int)Math.Pow(powerBase, tournamentSize);
            //int randomIndex = (rand() % maxIndex) + 1;
            //ga_struct selected = null;
            //for (int n=tournamentSize-1; n>=0; --n)
            //{
            //    if (randomIndex >= Math.Pow(powerBase, n))
            //    {
            //        selected = tournamentMembers[n];
            //        break;
            //    }
            //}

            sort_by_fitness(tournamentMembers);

            ga_struct selected = null;
            for (int i=0; i<tournamentSize; ++i)
            {
                if (_rand.NextDouble() < p)
                {
                    selected = tournamentMembers[i];
                    break;
                }
            }
            selected = selected ?? tournamentMembers[0];

            //Debug.Assert(selected != null);
            
            return selected;
        }*/

        private ga_struct select(GAVector population, int tournamentSize, double p)
        {
            ga_struct selected = null;
            int maxIncrement = GA_POPSIZE / tournamentSize;
            int index = rand() % maxIncrement;
            for (int i = 0; i < tournamentSize; ++i)
            {
                if (_rand.NextDouble() < p)
                {
                    selected = population[index];
                    break;
                }
                index += rand() % maxIncrement;
            }
            selected = selected ?? population[0];

            //Debug.Assert(selected != null);

            return selected;
        }

        private void tournamentSelection(GAVector population, GAVector buffer)
        {
            const int TOURNAMENT_SIZE = 15;    // GA_POPSIZE / 2;
            const double PROBABILITY_P = .50;

            ga_struct chromosome1, chromosome2;

            for (int i = 0; i < GA_POPSIZE; ++i)
            {
                chromosome1 = select(population, TOURNAMENT_SIZE, PROBABILITY_P);
                chromosome2 = select(population, TOURNAMENT_SIZE, PROBABILITY_P);

                buffer[i].str = combine(chromosome1, chromosome2);

                if (rand() < GA_MUTATION) mutate(buffer[i]);
            }        
        }

        private StringBuilder combine(ga_struct chromo1, ga_struct chromo2)
        {
            int tsize = GA_TARGET.Length, spos;

            spos = rand() % tsize;
            return new StringBuilder(chromo1.str.ToString().Substring(0, spos) + chromo2.str.ToString().Substring(spos, tsize - spos));
        }

        //private void elitism(GAVector population, GAVector buffer, int esize)
        private void elitism(GAVector population, GAVector buffer, int esize, bool mutateElite)
        {
            for (int i=0; i<esize; ++i)
            {
                buffer[i].str = population[i].str;
                buffer[i].fitness = population[i].fitness;

                // I added the following statement to mutate even the most-fit solutions (seems to help with convergence)
                if (mutateElite == true && rand() < GA_MUTATION) mutate(buffer[i]);
            }
        }

        private void mutate(ga_struct member)
        {
            int tsize = GA_TARGET.Length;
            int ipos = rand() % tsize;
            int delta = (rand() % 90) + 32;

            member.str[ipos] = (char)((member.str[ipos] + delta) % 122);
        }

        private void truncationSelection(GAVector population, GAVector buffer)
        {
            int esize = (int)(GA_POPSIZE * GA_ELITRATE);
            int tsize = GA_TARGET.Length, spos, i1, i2;

            elitism(population, buffer, esize, false);

            // Mate the rest
            for (int i=esize; i<GA_POPSIZE; ++i)
            {
                i1 = rand() % (GA_POPSIZE / 2);
                i2 = rand() % (GA_POPSIZE / 2);
                spos = rand() % tsize;

                buffer[i].str = new StringBuilder(population[i1].str.ToString().Substring(0, spos) + population[i2].str.ToString().Substring(spos, tsize - spos));

                if (rand() < GA_MUTATION) mutate(buffer[i]);
            }
        }

        private void print_best(GAVector gav, uint printCount)
        {
            for (int i=0; i<Math.Min(printCount, gav.Count); ++i)
            {
                Console.WriteLine("Best: {0} ({1})", gav[i].str, gav[i].fitness);                
            }
        }

        private void print_best(GAVector gav)
        {
            print_best(gav, 1);
            //Console.WriteLine("Best: {0} ({1})", gav[0].str, gav[0].fitness);
        }

        private void swap(ref GAVector population, ref GAVector buffer)
        {
            GAVector temp = population;
            population = buffer;
            buffer = temp;
        }

        private int rand()
        {
            return _rand.Next(RAND_MAX);
        }

        public uint Run(bool printMessages)
        {
            GAVector pop_alpha = new GAVector(), pop_beta = new GAVector();
            GAVector population, buffer;

            init_population(pop_alpha, pop_beta);
            population = pop_alpha;
            buffer = pop_beta;

            uint iterationCount = 0;
            string solutionMessage = null;
            for (uint i=0; i<GA_MAXITER; ++i)
            {
                calc_fitness(population);
                sort_by_fitness(population);
                if (printMessages == true)
                {
                    Console.Write("{0}  ", i + 1);
                    print_best(population);
                }

                if (population[0].fitness == 0)
                {
                    iterationCount = i + 1;
                    solutionMessage = string.Format("*** solution in  {0}  iterations", i);
                    break;
                }

                //truncationSelection(population, buffer);
                tournamentSelection(population, buffer);

                //calc_fitness(population);
                calc_fitness(buffer);
                swap(ref population, ref buffer);
                //print_best(population, 20);
            }

            if (printMessages == true)
            {
                Console.WriteLine();
                Console.WriteLine(solutionMessage ?? "*** NO SOLUTION FOUND.");
                Console.WriteLine();
            }

            return iterationCount;
        }
    } // END OF CLASS GAHelloWorld


    public class GAVector : System.Collections.Generic.List<Genetic_Algorithms.ga_struct>
    {
        public ulong Summary
        {
            get
            {
                ulong result = 0;
                for (int i = 0; i < this.Count; ++i)
                {
                    result += (ulong)(i * this[i].fitness);
                }
                return result;
            }
        }

        public void resize(uint size)
        {
            this.Clear();
            for (int i = 0; i < size; ++i)
            {
                this.Add(new ga_struct());
            }
        }

    }

    public class ga_struct
    {
        public StringBuilder str = new StringBuilder();         // the string
        public uint fitness;                                    // its fitness

        public override string ToString()
        {
            return string.Format("{0}:{1}", fitness, str);
        }
    }


} // END OF NAMESPACE
