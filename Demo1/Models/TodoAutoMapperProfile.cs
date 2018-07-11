using Demo1.Data.Entity;

namespace Demo1.Models
{
    public class TodoAutoMapperProfile : AutoMapper.Profile
    {
        public TodoAutoMapperProfile()
        {
            CreateMap<TodoEntity, Todo>()
                .AfterMap<TodoMapingAction>();

            CreateMap<TodoEntity, EditTodo>();
        }
    }
}