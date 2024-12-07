using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace QTool.View.Models
{
    public class QModuleInfo
    {

        public int Id { get; }

        public QPlatform Platform { get; }

        public QModule Module { get; }

        public string Title { get; }

        public object Args { get; }

        public bool IsHot { get; }

        private QModuleInfo(int id, QPlatform platform, QModule module, string title, object agrs, bool isHot = false)
            : this(id, platform, module, isHot)
        {
            Title = title;
            Args = agrs;
        }

        private QModuleInfo(int id, QPlatform platform, QModule module, bool isHot = false)
        {
            Id = id;
            Platform = platform;
            Module = module;
            Title = module.ToDisplayName();
            IsHot = isHot;
        }

        public static ReadOnlyCollection<QModuleGroup> Groups { get; } = CreateGroups();

        private static ReadOnlyCollection<QModuleGroup> CreateGroups()
        {
            return new ReadOnlyCollection<QModuleGroup>(new QModuleGroup[]
            {
                new QModuleGroup("速卖通"
                    , new QModuleInfo(100, QPlatform.AliExpress, QModule.AeBrowser,"速卖通后台","https://csp.aliexpress.com/")
                )
            });
        }

        public static QModuleInfo Find()
        {
            foreach (var group in Groups)
            {
                foreach (var module in group.Modules)
                {
                    return module;
                }
            }
            return null;
        }

        public static QModuleInfo FindByModuleId(int moduleId)
        {
            foreach (var group in Groups)
            {
                foreach (var module in group.Modules)
                {
                    if (module.Id == moduleId)
                    {
                        return module;
                    }
                }
            }
            return null;
        }
    }
}
