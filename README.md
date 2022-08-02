# Behavioral Analysis of PSI-Agents in a Multi-Agent Simulation of two Adversarial Groups

This simulation was developed during the creation of my bachelor thesis at the Freie Universität Berlin. It was designed according to the PSI-Theory that Dörner introduced in 1999.
During the development, I made several adaptions to the original formulation. A detailed description of the simulation can be found in my thesis.

For the execution, you will need Unity. I have used version 2021.3.0f1.

### Adjusting the Simulation

Settings about the simulation can be modified at three main points:

1. In the `SimulationSettings.cs`, where you can edit settings like the initial social scores.
2. In the World object of the unity editor, where you can set the world.json and agent generation and simulation seed.
3. In the Agent-Controller of the unity editor, where you can adjust the number of agents that will be spawned in the run.

### Controlling the Simulation

Some useful key and key-combinations to control the simulation.

| Key-Combination | Usage                                                                                                |
|-----------------|------------------------------------------------------------------------------------------------------|
| Z               | Open dialog to jump to a certain time-step. Important: It is only possible to jump forwards in time. |
| Ctrl + E        | Export agent information to file.                                                                    |
| Ctrl + O        | Open location where information is saved.                                                            |

### Artwork
Steven Colling provided the artwork. In addition, he gave the right to distribute necessary art for this thesis. If you are interested in using his artwork, you can find it on his [itch.io](https://stevencolling.itch.io/) page. Special thanks again to Steven!