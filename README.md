# ASCII Flappy Bird

The goal of this project was to replicate the once-popular "Flappy Bird" game in a console application form, using ASCII symbols for visuals. The application is made entirely in C# using only standard libraries.

![Game Preview](GameShowcase.gif)

The GIF above shows gameplay demonstration. The actual in-game framerate is much higher then GIF allows and does not have screen tearing.

The main challenges of the project were achieving stable image output without flickering at a high frame rate, implementing input handling, and creating a recognizable image using text characters.

## Features

* **ASCII-based Graphics:** The game is displayed entirely using ASCII characters.
* **High performance:** Runs at a stable frame rate of over 60 FPS.
* **Multithreading:** The game uses two separate threads in a way similar to modern game engines:
    * **Update** thread is responsible for input handling and draw calls.
    * **FixedUpdate** thread handles physics and movement calculations and runs at a a constant rate of 50 Hz. 
* **Subsymbol Precision:** The birds movement is achieved not only by moving a character up and down but also by using different letters to better match bird's actual position. The letter "b" is used when the bird is closer to the bottom of the character cell and "P" is used when it is closer to the top, achieving subsymbol accuracy.
* **Increasing Difficulty:** The game speeds up the longer you survive.
* **Score:** Score is shown at the end of each round.

## How to Play

1. Run **FlappyBirb.exe**.
2. Press any key to make the bird "flap" and rise.
3. Avoid obstacles as the game progressively speeds up.
4. Press Enter to restart the game after losing.

The game ends when you collide with an obstacle or fly outside the screen. Your score will be displayed, and the game can be replayed indefinitely.