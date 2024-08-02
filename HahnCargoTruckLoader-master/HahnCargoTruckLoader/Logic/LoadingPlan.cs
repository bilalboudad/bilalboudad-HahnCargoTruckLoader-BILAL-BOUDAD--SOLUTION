using HahnCargoTruckLoader.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HahnCargoTruckLoader.Logic
{
    public class LoadingPlan
    {
        private readonly Truck truck;
        private readonly List<Crate> crates;
        private readonly Dictionary<int, LoadingInstruction> instructions;

        public LoadingPlan(Truck truck, List<Crate> crates)
        {
            this.truck = truck;
            this.crates = crates;
            this.instructions = new Dictionary<int, LoadingInstruction>();
        }

        public Dictionary<int, LoadingInstruction> GetLoadingInstructions()
        {
            var cargoSpace = new bool[truck.Width, truck.Height, truck.Length];
            var sortedCrates = crates.OrderByDescending(c => c.Width * c.Height * c.Length).ToList(); // Sort crates by volume

            foreach (var crate in sortedCrates)
            {
                bool placed = false;

                foreach (var orientation in GetAllOrientations())
                {
                    // Apply orientation to crate
                    var orientedCrate = new Crate
                    {
                        CrateID = crate.CrateID,
                        Width = crate.Width,
                        Height = crate.Height,
                        Length = crate.Length
                    };
                    orientedCrate.Turn(orientation);

                    if (TryPlaceCrate(orientedCrate, orientation, cargoSpace, out var position))
                    {
                        instructions[crate.CrateID] = new LoadingInstruction
                        {
                            LoadingStepNumber = instructions.Count + 1,
                            CrateId = crate.CrateID,
                            TopLeftX = position.X,
                            TopLeftY = position.Y,
                            TurnHorizontal = orientation.TurnHorizontal,
                            TurnVertical = orientation.TurnVertical
                        };
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    Console.WriteLine($"Failed to place crate ID {crate.CrateID}. Checking next crate...");
                }
            }

            if (instructions.Count != crates.Count)
            {
                throw new Exception("Could not place all crates in the truck. Some crates were not placed.");
            }

            return instructions;
        }

        private IEnumerable<LoadingInstruction> GetAllOrientations()
        {
            return new List<LoadingInstruction>
            {
                new LoadingInstruction { TurnHorizontal = false, TurnVertical = false },
                new LoadingInstruction { TurnHorizontal = true, TurnVertical = false },
                new LoadingInstruction { TurnHorizontal = false, TurnVertical = true },
                new LoadingInstruction { TurnHorizontal = true, TurnVertical = true }
            };
        }

        private bool TryPlaceCrate(Crate crate, LoadingInstruction instruction, bool[,,] cargoSpace, out (int X, int Y, int Z) position)
        {
            position = (-1, -1, -1);

            int width = crate.Width;
            int height = crate.Height;
            int length = crate.Length;

            // Check all possible positions in the cargo space
            for (int x = 0; x <= truck.Width - width; x++)
            {
                for (int y = 0; y <= truck.Height - height; y++)
                {
                    for (int z = 0; z <= truck.Length - length; z++)
                    {
                        if (CanPlaceCrateAtPosition(x, y, z, width, height, length, cargoSpace))
                        {
                            position = (x, y, z);
                            MarkCargoSpace(x, y, z, width, height, length, cargoSpace);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool CanPlaceCrateAtPosition(int x, int y, int z, int width, int height, int length, bool[,,] cargoSpace)
        {
            for (int dx = x; dx < x + width; dx++)
            {
                for (int dy = y; dy < y + height; dy++)
                {
                    for (int dz = z; dz < z + length; dz++)
                    {
                        if (dx >= truck.Width || dy >= truck.Height || dz >= truck.Length || cargoSpace[dx, dy, dz])
                        {
                            return false; // Space is out of bounds or already occupied
                        }
                    }
                }
            }

            return true;
        }

        private void MarkCargoSpace(int x, int y, int z, int width, int height, int length, bool[,,] cargoSpace)
        {
            for (int dx = x; dx < x + width; dx++)
            {
                for (int dy = y; dy < y + height; dy++)
                {
                    for (int dz = z; dz < z + length; dz++)
                    {
                        cargoSpace[dx, dy, dz] = true;
                    }
                }
            }
        }
    }
}
