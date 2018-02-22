using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Domain.Models
{
    public interface IKeyedEntity<TKey>
    {
        TKey Id { get; set; }
    }
}