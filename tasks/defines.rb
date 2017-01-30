module BasicUnity

  class Defines < Thor
    namespace :defines
    include Thor::Actions
    include BasicUnity::BasicUnityHelper

    # adds :quiet, :skip, :pretent, :force
    add_runtime_options!

    desc "save", "report current scripting define symbols from ProjectSettings and save them to ./.defines"
    def save
      content = read_scripting_defines
      filename = File.join(ROOT_FOLDER, ".defines")
      File.open(filename, 'w') { |file| file.write(content) }
      say_status "saved", content, :green
    end

    desc "restore", "read ./.defines and write them to ProjectSettings"
    def restore
      filename = File.join(ROOT_FOLDER, ".defines")
      content = nil
      File.open(filename, "r") do |f|
        content = f.read.strip
      end

      write_scripting_defines(content)
      say_status "restored", content, :green
    end

  end
end
