using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IHasCardTags
{
    List<Card.Tag> Tags { get; }
}
