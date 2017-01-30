module BasicUnity

  class Defines < Thor
    namespace :defines
    include Thor::Actions
    include BasicUnity::BasicUnityHelper

    # adds :quiet, :skip, :pretent, :force
    add_runtime_options!

    desc "save", "report current scripting define symbols from ProjectSettings and save them to ./.defines"
    def save
      defines = read_scripting_defines
      filename = File.join(ROOT_FOLDER, ".defines")
      File.open(filename, 'w') { |file| file.write(defines) }
      say defines
      say "defines saved", :green
    end

    desc "restore", "read ./.defines and write them to ProjectSettings"
    def restore
      filename = File.join(ROOT_FOLDER, ".defines")
      contents = nil
      File.open(filename, "r") do |f|
        contents = f.read.strip
      end

      write_scripting_defines(contents)
      say "defines restored", :green
    end

  end
end
