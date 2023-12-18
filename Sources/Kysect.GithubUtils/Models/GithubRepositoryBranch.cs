namespace Kysect.GithubUtils.Models;

public readonly record struct GithubRepositoryBranch(string Owner, string Name, string Branch)
{
    public GithubRepositoryBranch(GithubRepository repository, string branch)
        : this(repository.Owner, repository.Name, branch)
    {
    }

    public GithubRepository GetRepository()
    {
        return new GithubRepository(Owner, Name);
    }

    public override string ToString()
    {
        return $"{Owner}/{Name}, Branch: {Branch}";
    }
}