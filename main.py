import os
from kivy.app import App
from kivy.core.window import Window
from flattenPdf import unlockPdf

from kvConfig import drag_widget as kv


class TestDropApp(App):
    def build(self):
        Window.bind(on_drop_file=self._on_drop_file)
        return

    def _on_drop_file(self, window, file_path, x, y):
        # try:
        output = unlockPdf(file_path)
        os.startfile(output)
        # except Exception as e:
        #     print(e)


if __name__ == "__main__":
    TestDropApp().run()
