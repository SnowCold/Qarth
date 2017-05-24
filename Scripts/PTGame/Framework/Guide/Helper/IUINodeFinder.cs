using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public interface IUINodeFinder : IRuntimeParam
	{
		Transform FindNode(bool search);
	}
}

