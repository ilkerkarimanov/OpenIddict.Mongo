using System.Collections.Generic;

namespace IDDD.Core
{

    public interface IFailure
    {
        IEnumerable<string> Errors { get; }
    }
    public interface IResult: IFailure
    {
        bool Succeeded { get; }

        string ToString();
    }
}