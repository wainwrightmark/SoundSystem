namespace SoundSystem;


public  readonly partial record struct Term(Cluster Cluster, NoteLength NoteLength)
{
    public string ABCName() => Cluster.ABCName() + NoteLength.ToABCString();
}