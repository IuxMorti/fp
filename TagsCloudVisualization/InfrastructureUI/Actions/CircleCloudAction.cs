﻿using System.Drawing;
using System.Windows.Forms;
using TagsCloudVisualization.Infrastructure;
using TagsCloudVisualization.Infrastructure.Algorithm.Curves;
using TagsCloudVisualization.InfrastructureUI.Painters;

namespace TagsCloudVisualization.InfrastructureUI.Actions
{
    public class CircleCloudAction : IUiAction
    {
        private readonly IImageHolder imageHolder;
        private readonly CloudPainter painter;
        private readonly SetTextAction setTextAction;

        public CircleCloudAction(CloudPainter painter,
            IImageHolder imageHolder, SetTextAction setTextAction)
        {
            this.setTextAction = setTextAction;
            this.painter = painter;
            this.imageHolder = imageHolder;
        }

        public Category Category => Category.Cloud;
        public string Name => "Окружность";
        public string Description => "";

        public void Perform()
        {
            var dialog = setTextAction.FileDialog();
            var result = dialog.ShowDialog();

            if (result != DialogResult.OK) return;

            var path = dialog.FileName;
            var size = imageHolder.GetImageSize().OnFail(Error.HandleError<ErrorHandlerUi>);
            if (!size.IsSuccess)
                return;
            var spiral = new Spiral(new Point(size.Value.Width / 2, size.Value.Height / 2));
            painter.Paint(path, spiral).OnFail(Error.HandleError<ErrorHandlerUi>);
        }
    }
}