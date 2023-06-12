using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.Domain.Entities.Base
{
    //
    public abstract class Entity : IEntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; protected set; }

        //Bu kod sayesinde ilgili entity eriştiğimizde clone'larak yeni nesne elde ediyor
        public Entity Clone()
        {
            return (Entity)this.MemberwiseClone();
        }
    }
}
