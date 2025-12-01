VAR User = "Player"
VAR PriestName = "Mentor"
EXTERNAL RequestScene(name)



=== opening_cinematic ===

PRIEST: {User}, you’ve served under me for three years now. I believe it’s time you tended a shrine of your own.

PRIEST: There is a small shrine along the coast. Namima. The villagers have neglected it ever since the old keeper passed on. It needs care… and perhaps _so do you_.

USER: A shrine of my own?

PRIEST: Yes. You would be its sole caretaker. Use the offerings wisely, honor the Kami, and remember to look after yourself as well.

USER: I’ll try… but, {PriestName}… what if I see _him_ again?

PRIEST: (sighs) Not this again, {User}.

PRIEST: What you claim to have seen is impossible. Dreams, shadows, tricks of the fog, nothing more.

PRIEST: Time away from the capital will settle your thoughts. Now go. It is a long road to Namima.

+ Go to shrine
    ~ RequestScene("OutdoorsScene")
    -> END







