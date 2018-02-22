using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Domain.Models
{
    public abstract class EntityBase<TKey> : IKeyedEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        [Column(Order = 0)]
        public TKey Id { get; set; }

        public bool IsNew
        {
            get
            {
                return this.Id.Equals(default(TKey));
            }
        }
    }
}