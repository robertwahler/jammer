module BasicUnity

  class Unity < Thor
    namespace :unity
    include Thor::Actions
    include BasicUnity::BasicUnityHelper

    # adds :quiet, :skip, :pretent, :force
    add_runtime_options!

    class_option :product, :type => :string, :aliases => "-p",:desc => "Set the product code"

    # The best way to get a consistent build is to force Unity to regenerate everything from scratch
    desc "reset", "rm -rf the ./Library folder, forces Unity to rebuild all"
    def reset
      folder = (File.join(ROOT_FOLDER, "Library"))
      remove_dir(folder)

      folder = (File.join(ROOT_FOLDER, "obj"))
      remove_dir(folder)

      folder = (File.join(ROOT_FOLDER, "Temp"))
      remove_dir(folder)
    end

    desc "IDE", "Run the Unity IDE interactively based on the contents '.unity'"
    def IDE
      cmd = "open /Applications/#{unity_binary_folder}/Unity.app"
      run(cmd)
    end

    desc "read", "read and echo the contents of '.unity'"
    def read
      say_status "current", unity_binary_folder
    end

    desc "set5_4", "configure environment for Unity 5.4"
    def set5_4
      set_instance_variables

      version = 'Unity.5.4'
      say_status :write, "setting '.unity' contents to #{version}"
      write_unity_version(version)

      say_status :warning, "consider running `thor unity:reset` when moving between non-patch releases", :yellow
    end

    desc "default", "configure environment for default Unity"
    def default
      set_instance_variables

      # remove NUnit dll/meta files from Asset, they are already include in 5.3+
      Dir.glob( File.join(ASSETS_FOLDER, "Test", 'Tools', 'NUnit', '*.dll*')  ).each do |filename|
        say_status :remove, "rm #{filename}"
        FileUtils.rm(filename)
      end

      version = 'Unity'
      say_status :write, "setting '.unity' contents to #{version}"
      write_unity_version(version)

      say_status :warning, "consider running `thor unity:reset` when moving between non-patch releases", :yellow
    end

    private

    # where to start looking for templates, required by the template methods
    # even though we are using absolute paths
    def self.source_root
      File.dirname(__FILE__)
    end

    def set_instance_variables
      @product = options[:product] ? options[:product]: default_product
    end

  end
end
