import os
from threading import Thread
from kivy.app import App
from kivy.graphics import Color, Rectangle
from kivy.uix.widget import Widget
from kivy.uix.image import Image
from kivy.uix.label import Label
from kivy.uix.floatlayout import FloatLayout
from kivy.lang.builder import Builder
from kivy.core.window import Window
from kivy.config import Config
from kivy.clock import Clock
from kivy.animation import Animation
from kivy.event import EventDispatcher
from kivy.properties import StringProperty
from flattenPdf import unlockPdf
from functools import partial

Config.set('graphics', 'width', '300')
Config.set('graphics', 'height', '300')

class OutputPath(EventDispatcher):
    value = StringProperty("")

class RootWidget(FloatLayout):
    def __init__(self, **kwargs):
        super(RootWidget, self).__init__(**kwargs)
        with self.canvas.before:
            Color(1,1,1,1)
            self.rect = Rectangle(pos=self.pos, size=self.size)
        self.bind(size=self.update_size)
        Window.bind(on_drop_file=self._on_drop_file)
        self.padlock = Image(source="locked-padlock.png", size_hint=(0.25,0.25), pos_hint={'center_x': 0.5, 'center_y': 0.5})
        self.add_widget(self.padlock)
        self.text = Label(text="Drop PDF here to unlock.", size_hint=(1, 0.1), pos_hint={'center_x': 0.5, 'center_y': 0.3}, color=(0,0,0,1))
        self.add_widget(self.text)
    
    def update_size(self, instance, value):
        self.rect.pos = self.pos
        self.rect.size = self.size
    def _on_drop_file(self, window, file_path, x, y):
        self.padlock.source = "spinner.gif"
        self.padlock.size_hint=(1,1)
        self.target_file_path = file_path
        output = OutputPath()
        output.value = ""
        output.bind(value=self.unlock_finished)
        thread = Thread(target=self.dispatch_unlock, args=(file_path, output))
        thread.start()
        
    def dispatch_unlock(self, file_path, output):
        output.value = unlockPdf(file_path)
        
    def unlock_finished(self, instance, value):
        Clock.schedule_once(partial(self.unlock_update, value), 1)
    
    def unlock_update(self, value, dt):
        self.padlock.source = "unlocked-padlock.png"
        self.padlock.size_hint=(0.25,0.25)
        os.startfile(value)
        
    def reset_ui(self):
        self.padlock.source = 'locked-padlock.png'


class TestDropApp(App):
    def build(self):
        Window.size = (400, 400)
        # Window.bind(on_drop_file=self._on_drop_file)
        Window.minimum_width, Window.minimum_height = (400, 200)
        return RootWidget()

if __name__ == "__main__":
    TestDropApp().run()
