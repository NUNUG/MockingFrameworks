﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MockingModels
{
	public interface IRepository<TEntity, TKey>
	{
		TEntity Find(TKey key);
		IEnumerable<TEntity> Find(TKey key, params TKey[] keys);
		TEntity Save(TEntity entity);
	}
}
