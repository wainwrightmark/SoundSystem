namespace SoundSystem;

public abstract record Mode(IReadOnlyList<int> Intervals)
{
    public IEnumerable<Chord> GetChordTypes(Tone keyNote)
        => GetChordTypesC().Select(x => x.Transpose(keyNote - Tone.C));

    protected abstract IEnumerable<Chord> GetChordTypesC();

    public sealed record Minor : Mode
    {
        private Minor() : base(new []{0,2,3,5,7,8,10}) { }

        public static Minor Instance { get; } = new ();

        /// <inheritdoc />
        protected override IEnumerable<Chord> GetChordTypesC()
        {
            return Major.Instance.GetChordTypes(Tone.Eb);
        }
    }

    public sealed record  Major : Mode
    {
        private Major() : base(new[]{0,2,4,5,7,9,11}) { }

        public static Major Instance { get; } = new ();

        /// <inheritdoc />
        protected override IEnumerable<Chord> GetChordTypesC()
        {
            yield return Chord.Major.TransposeToKey(Tone.C);
            yield return Chord.Major7.TransposeToKey(Tone.C);
            yield return Chord.MajorAdd2.TransposeToKey(Tone.C);
            yield return Chord.Dominant7.TransposeToKey(Tone.C);
            yield return Chord.Dominant7Sharp9.TransposeToKey(Tone.C);
            yield return Chord.Sus4.TransposeToKey(Tone.C);
            yield return Chord.Sus2.TransposeToKey(Tone.C);

            yield return Chord.Minor.TransposeToKey(Tone.D);
            yield return Chord.Minor7.TransposeToKey(Tone.D);

            yield return Chord.Minor.TransposeToKey(Tone.E);
            yield return Chord.Minor7.TransposeToKey(Tone.E);

            yield return Chord.Major.TransposeToKey(Tone.F);
            yield return Chord.MajorAdd2.TransposeToKey(Tone.F);
            yield return Chord.Major7.TransposeToKey(Tone.F);
            yield return Chord.Sus2.TransposeToKey(Tone.F);
            yield return Chord.Sus4.TransposeToKey(Tone.F);
            
            
            yield return Chord.Major.TransposeToKey(Tone.G);
            yield return Chord.MajorAdd2.TransposeToKey(Tone.G);
            yield return Chord.Dominant7.TransposeToKey(Tone.G);
            yield return Chord.Dominant7Sharp9.TransposeToKey(Tone.G);
            yield return Chord.Sus2.TransposeToKey(Tone.G);
            yield return Chord.Sus4.TransposeToKey(Tone.G);
            yield return Chord.Dominant11.TransposeToKey(Tone.G);

            
            yield return Chord.Minor.TransposeToKey(Tone.A);
            yield return Chord.Minor7.TransposeToKey(Tone.A);
            yield return Chord.Sus4.TransposeToKey(Tone.A);
            yield return Chord.Dominant11.TransposeToKey(Tone.A);

            yield return Chord.Diminished.TransposeToKey(Tone.B);
            yield return Chord.Diminished7.TransposeToKey(Tone.B);
            yield return Chord.HalfDiminished7.TransposeToKey(Tone.B);


            yield return Chord.Diminished7.TransposeToKey(Tone.Db);

            yield return Chord.Augmented.TransposeToKey(Tone.Eb);
            yield return Chord.Augmented7.TransposeToKey(Tone.Eb);


            yield return Chord.Augmented.TransposeToKey(Tone.Gb);
            yield return Chord.Augmented7.TransposeToKey(Tone.Gb);

            yield return Chord.Dominant7.TransposeToKey(Tone.Ab);
            yield return Chord.Major7.TransposeToKey(Tone.Ab);

            yield return Chord.Major.TransposeToKey(Tone.Bb);
            yield return Chord.MajorAdd2.TransposeToKey(Tone.Bb);
            yield return Chord.Major7.TransposeToKey(Tone.Bb);
            yield return Chord.Sus2.TransposeToKey(Tone.Bb);

        }
    }
}