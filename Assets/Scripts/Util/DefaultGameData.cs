using System.Collections.Generic;

public static class DefaultGameData
{
    public static List<AbstractSingle> AllPatterns = new List<AbstractSingle>()
    {
        new Nonspell2(),
        new Pattern01(){IsSpellCard = true},

        new MokouNon1(),
        new Nonspell3(){IsSpellCard = true},

        new Nonspell8(),
        new Nonspell4(){IsSpellCard = true},

        new Nonspell10(),
        new StarSpell1(){IsSpellCard = true},

        new Nonspell12(),
        new OldtroxSpell(){IsSpellCard = true},

        new Nonspell11(),
        new Nonspell6(){IsSpellCard = true},

        new Nonspell13(),
        new ShapeSpell(){IsSpellCard = true},

        new Nonspell5(),
        new SurroundSpell1(){IsSpellCard = true},

        new Nonspell7(){IsSpellCard = true},

        new Nonspell9(){IsSpellCard = true},
    };

    public static AbstractShot[] AllShots = new AbstractShot[] {
        new Shot1(),
        new Shot2(),
        new MarisaShot1(),
    };
}