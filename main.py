import os
from threading import Thread
from kivy.app import App
from kivy.graphics import Color, Rectangle
from kivy.uix.widget import Widget
from kivy.uix.image import Image
from kivy.uix.label import Label
from kivy.uix.floatlayout import FloatLayout
from kivy.uix.button import Button
from kivy.lang.builder import Builder
from kivy.core.window import Window
from kivy.config import Config
from kivy.clock import Clock
from kivy.animation import Animation
from kivy.event import EventDispatcher
from kivy.properties import StringProperty
from flattenPdf import unlockPdf
from functools import partial
import sys
from kivy.resources import resource_add_path, resource_find

Config.set('graphics', 'width', '300')
Config.set('graphics', 'height', '300')

locked_padlock = 'locked-padlock.png'
unlocked_padlock = 'unlocked-padlock.png'
spinner = 'spinner.gif'


class OutputPath(EventDispatcher):
    value = StringProperty("")


class RootWidget(FloatLayout):
    def __init__(self, **kwargs):
        super(RootWidget, self).__init__(**kwargs)
        with self.canvas.before:
            Color(1, 1, 1, 1)
            self.rect = Rectangle(pos=self.pos, size=self.size)
        self.bind(size=self.update_size)
        Window.bind(on_drop_file=self._on_drop_file)
        self.padlock = Image(source=locked_padlock, size_hint=(
            0.25, 0.25), pos_hint={'center_x': 0.5, 'center_y': 0.55})
        self.add_widget(self.padlock)
        self.text = Label(text="Drop PDF here to unlock.", size_hint_y=None, text_size=(
            self.width * 5, None), pos_hint={'center_x': 0.5, 'center_y': 0.35}, color=(0, 0, 0, 1), halign="center")
        self.text.height = self.text.texture_size[1]
        self.add_widget(self.text)
        self.button = Button(text="Open", size_hint=(0.25, 0.1), pos_hint={
                             'center_x': 0.5, 'center_y': 0.15})
        self.button.bind(on_press=self.open_file)

    def update_size(self, instance, value):
        self.rect.pos = self.pos
        self.rect.size = self.size

    def _on_drop_file(self, window, file_path, x, y):
        self.padlock.source = spinner
        self.padlock.size_hint = (1, 1)
        self.target_file_path = file_path
        self.text.text = ""
        output = OutputPath()
        output.value = ""
        output.bind(value=self.unlock_finished)
        thread = Thread(target=self.dispatch_unlock, args=(file_path, output))
        thread.start()

    def dispatch_unlock(self, file_path, output):
        output.value = unlockPdf(file_path)
        self.output_file_path = output.value

    def unlock_finished(self, instance, value):
        Clock.schedule_once(partial(self.unlock_update, value), 1)

    def unlock_update(self, value, dt):
        self.padlock.source = unlocked_padlock
        self.text.text = self.output_file_path
        self.padlock.size_hint = (0.25, 0.25)
        self.add_widget(self.button)

    def open_file(self, instance,):
        os.startfile(self.output_file_path)

    def reset_ui(self):
        self.padlock.source = 'locked-padlock.png'


class PdfUnlockApp(App):
    def build(self):
        Window.size = (400, 400)
        # Window.bind(on_drop_file=self._on_drop_file)
        Window.minimum_width, Window.minimum_height = (400, 200)
        return RootWidget()


if __name__ == "__main__":
    if hasattr(sys, '_MEIPASS'):
        resource_add_path(os.path.join(sys._MEIPASS, 'assets'))
        print(sys._MEIPASS)
    resource_add_path('./assets')
    app = PdfUnlockApp()
    app.run()
