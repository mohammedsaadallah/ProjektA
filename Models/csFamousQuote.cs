using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Linq;

using Configuration;

namespace Models
{
    public class csFamousQuote
    {
        public Guid QuoteId {get; set;} = Guid.NewGuid();
        public string Quote { get; set; }
        public string Author { get; set; }

        public csFamousQuote() {}
        public csFamousQuote(csFamousQuote original)
        {
            QuoteId = original.QuoteId;
            Quote = original.Quote;
            Author = original.Author;
        }
    }
}