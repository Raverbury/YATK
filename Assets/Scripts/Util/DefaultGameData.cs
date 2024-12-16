public static class DefaultGameData
{
    public static readonly AbstractSingle[] AllPatterns = new AbstractSingle[]
    {
        new Stage1Chapter1(),

        new Nonspell2(),
        new Pattern01(),

        new MokouNon1(),
        new Nonspell3(),

        new Nonspell8(),
        new Nonspell4(),

        new Nonspell10(),
        new StarSpell1(),

        new Nonspell12(),
        new OldtroxSpell(),

        new Nonspell11(),
        new Nonspell6(),

        new Nonspell13(),
        new ShapeSpell(),

        new Nonspell5(),
        new SurroundSpell1(),

        new Nonspell7(),

        new Nonspell9(),
    };

    public static readonly AbstractShot[] AllShots = new AbstractShot[]
    {
        new Shot1(),
        new Shot2(),
        new MarisaShot1(),
    };

    public static readonly AbstractBombWeapon[] AllBombs = new AbstractBombWeapon[]
    {
        new MasterSparkBomb(),
        new FantasyOrbBomb(),
    };
}